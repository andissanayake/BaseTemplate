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
import { useNavigate } from "react-router-dom";
import { apiClient } from "../../common/apiClient";
import {
  SpecificationModel,
  SpecificationCreateModel,
} from "./SpecificationModel";
import { getAllSpecifications } from "./SpecificationUtils";

const { Title } = Typography;
const { TextArea } = Input;

const SpecificationCreate: React.FC = () => {
  const navigate = useNavigate();
  const [loading, setLoading] = useState(false);
  const [parentOptions, setParentOptions] = useState<
    { value: number; label: string }[]
  >([]);
  const [form] = Form.useForm();

  useEffect(() => {
    loadParentOptions();
  }, []);

  const loadParentOptions = () => {
    apiClient.get<{ specifications: SpecificationModel[] }>(
      "/api/specification",
      {
        onSuccess: (data) => {
          if (data?.specifications) {
            // Get all specifications recursively (including children)
            const allSpecs = getAllSpecifications(data.specifications);
            const options = allSpecs.map((spec) => ({
              value: spec.id,
              label: spec.fullPath || spec.name,
            }));
            setParentOptions(options);
          }
        },
        onServerError: () => {
          console.error("Failed to load parent options");
        },
      }
    );
  };

  const onFinish = (values: SpecificationCreateModel) => {
    setLoading(true);
    apiClient.post<number>("/api/specification", values, {
      onSuccess: () => {
        notification.success({
          message: "Specification created successfully!",
        });
        navigate("/specifications");
      },
      onServerError: () => {
        notification.error({ message: "Failed to create specification!" });
      },
      onFinally: () => {
        setLoading(false);
      },
    });
  };

  return (
    <Card>
      <div style={{ marginBottom: 16 }}>
        <Title level={3} style={{ margin: 0 }}>
          Create Specification
        </Title>
      </div>

      <Spin spinning={loading}>
        <Form
          form={form}
          layout="vertical"
          onFinish={onFinish}
          initialValues={{
            name: "",
            description: "",
            parentSpecificationId: undefined,
          }}
        >
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
            />
          </Form.Item>

          <Form.Item>
            <Button type="primary" htmlType="submit" loading={loading}>
              Create Specification
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

export default SpecificationCreate;
