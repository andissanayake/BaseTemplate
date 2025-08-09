import {
  useEffect,
  useState,
  forwardRef,
  useImperativeHandle,
  useCallback,
} from "react";
import { Form, Input, Select, notification } from "antd";
import { apiClient } from "../../common/apiClient";
import { useAsyncEffect } from "../../common/useAsyncEffect";
import { handleFormValidationErrors } from "../../common/formErrorHandler";
import { Item } from "./ItemModel";
import { SpecificationModel } from "../Specification/SpecificationModel";
import { getAllSpecifications } from "../Specification/SpecificationUtils";

interface ItemBasicInfoStepProps {
  itemId: string;
  onSaveSuccess: () => void;
  onLoadingChange: (loading: boolean) => void;
}

export interface ItemBasicInfoStepHandle {
  save: () => void;
}

const ItemBasicInfoStep = forwardRef<
  ItemBasicInfoStepHandle,
  ItemBasicInfoStepProps
>(({ itemId, onSaveSuccess, onLoadingChange }, ref) => {
  const [form] = Form.useForm();
  const [specificationOptions, setSpecificationOptions] = useState<
    { value: number; label: string }[]
  >([]);

  useEffect(() => {
    loadSpecifications();
  }, []);

  const loadSpecifications = () => {
    apiClient.get<{ specifications: SpecificationModel[] }>(
      "/api/specification",
      {
        onSuccess: (data) => {
          if (data?.specifications) {
            const allSpecs = getAllSpecifications(data.specifications);
            const options = allSpecs.map((spec) => ({
              value: spec.id,
              label: spec.fullPath || spec.name,
            }));
            setSpecificationOptions(options);
          }
        },
        onServerError: () => {
          notification.error({ message: "Failed to load specifications!" });
        },
      }
    );
  };

  const handleSave = useCallback(() => {
    form.validateFields().then(async (values) => {
      values.id = +itemId;
      values.tags = values.tags?.join(",") || "";
      onLoadingChange(true);

      apiClient.put<boolean>(`/api/item`, values, {
        onSuccess: () => {
          notification.success({
            message: "Item basic info updated successfully!",
          });
          onSaveSuccess();
        },
        onValidationError: (updateErrors) => {
          handleFormValidationErrors({
            form,
            errors: updateErrors,
          });
        },
        onServerError: () => {
          notification.error({ message: "Failed to save item!" });
        },
        onFinally: () => {
          onLoadingChange(false);
        },
      });
    });
  }, [form, itemId, onLoadingChange, onSaveSuccess]);

  useAsyncEffect(async () => {
    form.resetFields();
    onLoadingChange(true);

    apiClient.get<Item>(`/api/item/${itemId}`, {
      onSuccess: (data) => {
        if (data) {
          form.setFieldsValue({
            ...data,
            tags: data.tags ? data.tags.split(",").filter(Boolean) : [],
          });
        }
      },
      onServerError: () => {
        notification.error({ message: "Failed to fetch item!" });
      },
      onFinally: () => {
        onLoadingChange(false);
      },
    });
  }, [itemId, form]);

  // Expose save method to parent
  useImperativeHandle(
    ref,
    () => ({
      save: handleSave,
    }),
    [handleSave]
  );

  return (
    <Form form={form} layout="vertical">
      <Form.Item
        label="Name"
        name="name"
        rules={[{ required: true, message: "Please enter the item name!" }]}
      >
        <Input placeholder="Enter item name" />
      </Form.Item>

      <Form.Item label="Description" name="description">
        <Input.TextArea placeholder="Enter item description" />
      </Form.Item>

      <Form.Item
        label="Specification"
        name="specificationId"
        rules={[{ required: true, message: "Please select a specification!" }]}
      >
        <Select
          placeholder="Select specification"
          options={specificationOptions}
          showSearch
          filterOption={(input, option) =>
            (option?.label ?? "").toLowerCase().includes(input.toLowerCase())
          }
        />
      </Form.Item>

      <Form.Item label="Tags" name="tags">
        <Select
          mode="tags"
          style={{ width: "100%" }}
          placeholder="Enter tags"
          tokenSeparators={[","]}
        />
      </Form.Item>
    </Form>
  );
});

ItemBasicInfoStep.displayName = "ItemBasicInfoStep";

export { ItemBasicInfoStep };
