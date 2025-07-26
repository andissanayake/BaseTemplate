import React, { useEffect, useState } from "react";
import {
  Form,
  Input,
  Button,
  Card,
  Typography,
  Select,
  notification,
  Spin,
} from "antd";
import { useNavigate, useParams } from "react-router-dom";
import { apiClient } from "../../common/apiClient";
import {
  SpecificationModel,
  SpecificationUpdateModel,
} from "./SpecificationModel";

const { Title } = Typography;
const { TextArea } = Input;

const SpecificationEdit: React.FC = () => {
  const navigate = useNavigate();
  const { specificationId } = useParams<{ specificationId: string }>();
  const [loading, setLoading] = useState(false);
  const [initialLoading, setInitialLoading] = useState(true);
  const [parentOptions, setParentOptions] = useState<
    { value: number; label: string }[]
  >([]);
  const [form] = Form.useForm();

  useEffect(() => {
    if (specificationId) {
      loadParentOptions();
    }
  }, [specificationId]);

  const loadSpecification = () => {
    apiClient.get<SpecificationModel>(
      `/api/specifications/${specificationId}`,
      {
        onSuccess: (data) => {
          if (data) {
            form.setFieldsValue({
              name: data.name,
              description: data.description,
              parentSpecificationId: data.parentSpecificationId,
            });
          }
          setInitialLoading(false);
        },
        onServerError: () => {
          notification.error({ message: "Failed to load specification!" });
          setInitialLoading(false);
        },
      }
    );
  };

  const loadParentOptions = () => {
    apiClient.get<{ specifications: SpecificationModel[] }>(
      "/api/specifications",
      {
        onSuccess: (data) => {
          if (data?.specifications) {
            // Get all specifications recursively (including children)
            const allSpecs = getAllSpecifications(data.specifications);
            // Filter out the current specification and its children to prevent circular references
            const currentId = parseInt(specificationId || "0");
            const filteredSpecs = allSpecs.filter(
              (spec) =>
                spec.id !== currentId &&
                !isDescendant(spec, currentId, allSpecs)
            );

            const options = filteredSpecs.map((spec) => ({
              value: spec.id,
              label: spec.fullPath || spec.name,
            }));
            setParentOptions(options);
            console.log(options);
            // Now load the specification data after parent options are ready
            loadSpecification();
          }
        },
        onServerError: () => {
          console.error("Failed to load parent options");
          setInitialLoading(false);
        },
      }
    );
  };

  const getAllSpecifications = (
    specs: SpecificationModel[]
  ): SpecificationModel[] => {
    const allSpecs: SpecificationModel[] = [];

    const addSpecsRecursively = (specList: SpecificationModel[]) => {
      specList.forEach((spec) => {
        allSpecs.push(spec);
        if (spec.children && spec.children.length > 0) {
          addSpecsRecursively(spec.children);
        }
      });
    };

    addSpecsRecursively(specs);
    return allSpecs;
  };

  const isDescendant = (
    spec: SpecificationModel,
    targetId: number,
    allSpecs: SpecificationModel[]
  ): boolean => {
    if (spec.id === targetId) return true;
    return spec.children.some((child) =>
      isDescendant(child, targetId, allSpecs)
    );
  };

  const onFinish = (values: SpecificationUpdateModel) => {
    if (!specificationId) return;

    setLoading(true);
    const updateData: SpecificationUpdateModel = {
      ...values,
      id: parseInt(specificationId),
    };

    apiClient.put<boolean>(
      `/api/specifications/${specificationId}`,
      updateData,
      {
        onSuccess: () => {
          notification.success({
            message: "Specification updated successfully!",
          });
          navigate("/specifications");
        },
        onServerError: () => {
          notification.error({ message: "Failed to update specification!" });
        },
        onFinally: () => {
          setLoading(false);
        },
      }
    );
  };

  if (initialLoading) {
    return (
      <Card>
        <div style={{ textAlign: "center", padding: "50px" }}>
          <Spin size="large" />
        </div>
      </Card>
    );
  }

  return (
    <Card>
      <div style={{ marginBottom: 16 }}>
        <Title level={3} style={{ margin: 0 }}>
          Edit Specification
        </Title>
      </div>

      <Spin spinning={loading}>
        <Form form={form} layout="vertical" onFinish={onFinish}>
          <Form.Item
            label="Name"
            name="name"
            rules={[
              { required: true, message: "Please enter specification name!" },
            ]}
          >
            <Input placeholder="Enter specification name" />
          </Form.Item>

          <Form.Item
            label="Description"
            name="description"
            rules={[
              {
                required: true,
                message: "Please enter specification description!",
              },
            ]}
          >
            <TextArea rows={4} placeholder="Enter specification description" />
          </Form.Item>

          <Form.Item label="Parent Specification" name="parentSpecificationId">
            <Select
              placeholder="Select parent specification (optional)"
              allowClear
              options={parentOptions}
              labelInValue={false}
            />
          </Form.Item>

          <Form.Item>
            <Button type="primary" htmlType="submit" loading={loading}>
              Update Specification
            </Button>
            <Button
              style={{ marginLeft: 8 }}
              onClick={() => navigate("/specifications")}
            >
              Cancel
            </Button>
          </Form.Item>
        </Form>
      </Spin>
    </Card>
  );
};

export default SpecificationEdit;
